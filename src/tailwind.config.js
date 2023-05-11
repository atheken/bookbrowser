module.exports = {
  safelist : () => process.env.ENABLE_TAILWINDCSS_PURGE || false ? ["all"] : [],
  content : [
    './**/*.razor',
    './**/*.cshtml'
  ],
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
