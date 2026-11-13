export interface AuditLogModel {
  id: number;
  userId: number | null;
  userName: string | null;
  action: string;
  entityName: string;
  entityId: string | null;
  oldValues: string | null;
  newValues: string | null;
  timestamp: string;
}
