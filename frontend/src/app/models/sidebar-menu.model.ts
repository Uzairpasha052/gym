export interface SidebarMenuItem {
  id: number;
  title: string;
  route: string;
  icon: string;
  displayOrder: number;
  parentId?: number | null;
  isActive?: boolean;
  roleIds?: number[];
}

export interface SidebarMenuCreateRequest {
  title: string;
  route: string;
  icon: string;
  parentId: number | null;
  displayOrder: number;
  roleIds: number[];
}

export interface SidebarMenuUpdateRequest extends SidebarMenuCreateRequest {
  isActive: boolean;
}
