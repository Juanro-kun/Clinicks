/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: "oklch(0.75 0.15 175)", // El verde esmeralda de tu diseño
      }
    },
  },
  plugins: [],
}