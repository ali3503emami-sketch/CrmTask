import { Menu } from 'antd'
import { menuItems, type PageKey } from './menuItems'

interface AppMenuProps {
  selectedKey: PageKey
  onSelect: (key: PageKey) => void
}

/**
 * The navigation Menu itself, shared between the always-visible desktop
 * Sider and the mobile hamburger Drawer (see App.tsx) — one definition so the
 * two surfaces can't drift apart.
 */
export function AppMenu({ selectedKey, onSelect }: AppMenuProps) {
  return (
    <Menu
      mode="inline"
      style={{ height: '100%', borderInlineEnd: 'none' }}
      defaultOpenKeys={['user-group', 'basic-info', 'customer-affairs']}
      selectedKeys={[selectedKey]}
      items={menuItems}
      onClick={({ key }) => onSelect(key as PageKey)}
    />
  )
}
