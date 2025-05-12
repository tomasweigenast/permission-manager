import type PermissionType from "./permission_type";

export default interface Permission {
  id: number;
  employee_name: string;
  employee_surname: string;
  permission_type: PermissionType;
  granted_at: string;
}
