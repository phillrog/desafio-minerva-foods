
// Common Styles and Design Tokens (Phillipe Roger Style)
export const colors = {
  // Primary Colors (Minerva Foods Brand)
  primary: '#2b5373',
  secondary: '#e84c53',
  accent: '#ececec',

  // Background Colors
  background: '#ffffff',
  backgroundSecondary: '#f5f5f5',
  card: '#ffffff',

  // Text Colors
  text: '#1a1a1a',
  textSecondary: '#666666',
  textLight: '#999999',

  // Status Colors (Enum-based)
  statusProcessing: '#FF9800', // 0: Processando (Orange)
  statusCreated: '#2196F3',    // 1: Criado (Blue)
  statusPaid: '#4CAF50',       // 2: Pago (Green)
  statusCancelled: '#F44336',  // 3: Cancelado (Red)
  statusError: '#9E9E9E',      // 9: Erro (Gray)

  // UI Colors
  border: '#e0e0e0',
  error: '#F44336',
  success: '#4CAF50',
  warning: '#FF9800',
  info: '#2196F3',
};

export const spacing = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
};

export const borderRadius = {
  sm: 4,
  md: 8,
  lg: 12,
  xl: 16,
  round: 999,
};

export const shadows = {
  sm: {
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 2,
    elevation: 1,
  },
  md: {
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.08,
    shadowRadius: 4,
    elevation: 2,
  },
  lg: {
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.12,
    shadowRadius: 8,
    elevation: 4,
  },
};
