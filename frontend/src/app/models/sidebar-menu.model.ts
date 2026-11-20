export interface SidebarMenuItem {
  id: number;
  title: string;
  route: string;
  icon: string;
  displayOrder: number;
  parentId?: number | null;
  isActive?: boolean;
  roleIds?: number[];
  userIds?: number[];
  isGroup?: boolean;
}

export interface SidebarMenuCreateRequest {
  title: string;
  route: string;
  icon: string;
  parentId: number | null;
  displayOrder: number;
  roleIds: number[];
  userIds: number[];
}

export interface SidebarMenuUpdateRequest extends SidebarMenuCreateRequest {
  isActive: boolean;
}
