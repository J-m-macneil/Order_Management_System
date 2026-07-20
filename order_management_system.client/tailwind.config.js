/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/**/*.{html,ts}',
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        // Primary brand colors
        blue: {
          600: '#2563eb',
          500: '#3b82f6',
          700: '#1d4ed8',
        },
        // Slate palette for backgrounds and text
        slate: {
          50: '#f8fafc',
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#1e293b',
          900: '#0f172a',
        },
        // Status colors
        emerald: {
          500: '#10b981',
        },
        amber: {
          500: '#f59e0b',
        },
        red: {
          500: '#ef4444',
        },
        purple: {
          500: '#a855f7',
        },
        // Hazard class colors
        orange: {
          500: '#f97316',
        },
        yellow: {
          400: '#facc15',
        },
        pink: {
          500: '#ec4899',
        },
      },
      fontSize: {
        xs: ['0.75rem', { lineHeight: '1rem' }],
        sm: ['0.875rem', { lineHeight: '1.25rem' }],
        base: ['1rem', { lineHeight: '1.5rem' }],
        lg: ['1.125rem', { lineHeight: '1.75rem' }],
        '2xl': ['1.5rem', { lineHeight: '2rem' }],
        '3xl': ['1.875rem', { lineHeight: '2.25rem' }],
      },
      fontWeight: {
        light: '300',
        normal: '400',
        medium: '500',
        semibold: '600',
      },
      letterSpacing: {
        wide: '0.05em',
      },
      maxWidth: {
        '1400': '1400px',
      },
      spacing: {
        '6': '1.5rem',
        '8': '2rem',
      },
      borderRadius: {
        md: '0.375rem',
        lg: '0.5rem',
      },
      boxShadow: {
        sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
      },
    },
  },
  plugins: [],
};
