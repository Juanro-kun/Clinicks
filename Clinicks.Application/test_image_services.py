import pytest
from io import BytesIO
from unittest.mock import MagicMock, patch
from app.services.image_services import subir_imagen
from app.utils.imagen_validators import (
    validar_extension_archivo,
    validar_nombre_archivo,
    validar_tamano_archivo,
    validar_archivo_en_request,
)

# ==================================================
# VALIDAR EXTENSIONES
# ==================================================

def test_subir_jpg_valido():
    assert validar_extension_archivo("foto.jpg") == 2


def test_subir_png_valido():
    assert validar_extension_archivo("foto.png") == 1


def test_subir_jpeg_valido():
    assert validar_extension_archivo("foto.jpeg") == 3


def test_subir_gif():
    with pytest.raises(ValueError) as excinfo:
        validar_extension_archivo("animacion.gif")

    assert str(excinfo.value) == (
        "La imagen debe tener formato (JPG/PNG/JPEG)"
    )


def test_subir_bmp():
    with pytest.raises(ValueError):
        validar_extension_archivo("imagen.bmp")


def test_subir_webp():
    with pytest.raises(ValueError):
        validar_extension_archivo("imagen.webp")


def test_subir_archivo_sin_extension():
    with pytest.raises(ValueError):
        validar_extension_archivo("archivo")


# ==================================================
# VALIDAR NOMBRE
# ==================================================

def test_subir_archivo_vacio():

    file = MagicMock()
    file.filename = ""

    with pytest.raises(ValueError) as excinfo:
        validar_nombre_archivo(file)

    assert str(excinfo.value) == (
        "Nombre de archivo inválido"
    )


def test_nombre_archivo_valido():

    file = MagicMock()
    file.filename = "foto.jpg"

    validar_nombre_archivo(file)


# ==================================================
# VALIDAR TAMAÑO
# ==================================================

def test_subir_imagen_0_1mb():

    contenido = BytesIO(
        b"a" * 100000
    )

    validar_tamano_archivo(contenido)


def test_subir_imagen_5mb():

    contenido = BytesIO(
        b"a" * (5 * 1024 * 1024)
    )

    validar_tamano_archivo(contenido)


def test_subir_imagen_mayor_5mb():

    contenido = BytesIO(
        b"a" * ((5 * 1024 * 1024) + 1)
    )

    with pytest.raises(ValueError) as excinfo:
        validar_tamano_archivo(contenido)

    assert str(excinfo.value) == (
        "Su imagen supera el límite de tamaño de 5MB"
    )


# ==================================================
# SUBIR IMAGEN
# ==================================================

@patch("app.services.image_services.insertar_imagen")
@patch("app.services.image_services.obtener_metadata_imagen")
def test_subir_imagen_exitosamente(
    mock_metadata,
    mock_insertar
):

    mock_metadata.return_value = (
        "2025-08-01",
        "2025-09-01",
        800,
        600,
        1024,
        "foto.jpg"
    )

    mock_insertar.return_value = {
        "id_imagen": 1,
        "altura": 800,
        "ancho": 600,
        "fecha_subida": "2025-08-01",
        "fecha_expiracion": "2025-09-01",
        "peso_subida": 1024,
        "ruta": "uploads/foto.jpg",
        "id_tipoimagen": 2,
        "nombre_archivo": "foto.jpg"
    }

    resultado = subir_imagen(
        "uploads/foto.jpg",
        1,
        2
    )

    assert resultado is not None
    assert resultado.id == 1
    assert resultado.nombre_archivo == "foto.jpg"


@patch("app.services.image_services.obtener_metadata_imagen")
def test_subir_imagen_corrupta(
    mock_metadata
):

    mock_metadata.return_value = None

    with pytest.raises(ValueError) as excinfo:
        subir_imagen(
            "imagen_corrupta.jpg",
            1,
            2
        )

    assert str(excinfo.value) == (
        "Ocurrió un error al intentar subir la imagen verifique que su imagen no esté dañada"
    )

# ==================================================
# CASO USUARIO INEXISTENTE
# ==================================================

@patch("app.services.image_services.insertar_imagen")
@patch("app.services.image_services.obtener_metadata_imagen")
def test_subir_usuario_inexistente(
    mock_metadata,
    mock_insertar
):

    mock_metadata.return_value = (
        "2025-08-01",
        "2025-09-01",
        800,
        600,
        1024,
        "foto.jpg"
    )

    mock_insertar.side_effect = Exception(
        "Usuario no encontrado"
    )

    with pytest.raises(Exception) as excinfo:
        subir_imagen(
            "uploads/foto.jpg",
            999,
            2
        )

    assert str(excinfo.value) == (
        "Usuario no encontrado"
    )


# ==================================================
# VALIDAR REQUEST
# ==================================================


def test_request_sin_archivo():

    with pytest.raises(ValueError) as excinfo:
        validar_archivo_en_request({})

    assert str(excinfo.value) == (
        "No se envió ningún archivo"
    )


def test_request_con_archivo():

    archivo = MagicMock()

    resultado = validar_archivo_en_request(
        {"file": archivo}
    )

    assert resultado == archivo

    # ==================================================
# USUARIO AUTENTICADO
# ==================================================

@patch("app.services.image_services.insertar_imagen")
@patch("app.services.image_services.obtener_metadata_imagen")
def test_subir_imagen_usuario_autenticado(
    mock_metadata,
    mock_insertar
):

    mock_metadata.return_value = (
        "2025-08-01",
        "2025-09-01",
        800,
        600,
        1024,
        "foto.jpg"
    )

    mock_insertar.return_value = {
        "id_imagen": 1,
        "altura": 800,
        "ancho": 600,
        "fecha_subida": "2025-08-01",
        "fecha_expiracion": "2025-09-01",
        "peso_subida": 1024,
        "ruta": "uploads/foto.jpg",
        "id_tipoimagen": 2,
        "nombre_archivo": "foto.jpg"
    }

    resultado = subir_imagen(
        "uploads/foto.jpg",
        1,
        2
    )

    assert resultado is not None

    # ==================================================
# VINCULACION CON USUARIO
# ==================================================

@patch("app.services.image_services.insertar_imagen")
@patch("app.services.image_services.obtener_metadata_imagen")
def test_imagen_se_vincula_con_usuario(
    mock_metadata,
    mock_insertar
):

    mock_metadata.return_value = (
        "2025-08-01",
        "2025-09-01",
        800,
        600,
        1024,
        "foto.jpg"
    )

    mock_insertar.return_value = {
        "id_imagen": 1,
        "altura": 800,
        "ancho": 600,
        "fecha_subida": "2025-08-01",
        "fecha_expiracion": "2025-09-01",
        "peso_subida": 1024,
        "ruta": "uploads/foto.jpg",
        "id_tipoimagen": 2,
        "nombre_archivo": "foto.jpg"
    }

    subir_imagen(
        "uploads/foto.jpg",
        55,
        2
    )

    args = mock_insertar.call_args[0][0]

    assert args[6] == 55

    # ==================================================
# PERSISTENCIA EN BASE DE DATOS
# ==================================================

@patch("app.services.image_services.insertar_imagen")
@patch("app.services.image_services.obtener_metadata_imagen")
def test_imagen_se_guarda_en_bd(
    mock_metadata,
    mock_insertar
):

    mock_metadata.return_value = (
        "2025-08-01",
        "2025-09-01",
        800,
        600,
        1024,
        "foto.jpg"
    )

    mock_insertar.return_value = {
        "id_imagen": 1,
        "altura": 800,
        "ancho": 600,
        "fecha_subida": "2025-08-01",
        "fecha_expiracion": "2025-09-01",
        "peso_subida": 1024,
        "ruta": "uploads/foto.jpg",
        "id_tipoimagen": 2,
        "nombre_archivo": "foto.jpg"
    }

    subir_imagen(
        "uploads/foto.jpg",
        1,
        2
    )

    mock_insertar.assert_called_once()