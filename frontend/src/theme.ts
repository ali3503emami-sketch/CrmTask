import { theme as antdTheme, type ThemeConfig } from 'antd'

/**
 * Compact + restrained token set for information-dense CRM screens.
 * See docs/frontend-design-system.md for the reasoning behind each choice.
 */
export const crmTheme: ThemeConfig = {
  algorithm: antdTheme.compactAlgorithm,
  token: {
    colorPrimary: '#2f6f6b',
    borderRadius: 6,
    fontSize: 13,
    fontFamily:
      "'Vazirmatn Variable', 'Vazirmatn', Tahoma, 'Segoe UI', sans-serif",
    colorBorderSecondary: '#e5e1d8',
    colorTextSecondary: '#6b6459',
    wireframe: false,
  },
  components: {
    Table: {
      cellPaddingBlock: 8,
      cellPaddingInline: 10,
      headerBg: '#f7f5ef',
    },
    Card: {
      paddingLG: 16,
    },
  },
}
