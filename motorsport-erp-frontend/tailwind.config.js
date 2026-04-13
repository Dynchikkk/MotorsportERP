/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        racing: {
          red: '#E63946',    
          dark: '#121212',   
          panel: '#1E1E1E',  
          gray: '#A0A0A0'    
        }
      }
    },
  },
  plugins: [],
}