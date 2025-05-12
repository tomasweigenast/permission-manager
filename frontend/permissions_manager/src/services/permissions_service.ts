import type IPagedList from "../data/paged_list";
import type Permission from "../data/permission";
import type PermissionType from "../data/permission_type";
import type IPermissionsResponse from "../data/permissions_response";
import api from "./api";

async function getPermissions({pageSize, page, permissionTypeId}: {pageSize?: number, page?: number, permissionTypeId?: number}): Promise<IPagedList<Permission>> {
    const searchParams = new URLSearchParams();
    if (pageSize) {
        searchParams.append("page_size", pageSize.toString());
    }
    if (page) {
        searchParams.append("page", page.toString());
    }
    if (permissionTypeId) {
        searchParams.append("permission_type_id", permissionTypeId.toString());
    }
    const url = `/permissions?${searchParams.toString()}`;

    const permissions = (await api.get(url)).data as IPermissionsResponse;
    console.log("result", permissions)
    return {
        hasMore: permissions.has_next_page,
        items: permissions.data,
    }
}

async function getPermissionTypes(): Promise<PermissionType[]> {
    const permissions = (await api.get("/permission_types")).data as PermissionType[];
    return permissions;
}

async function requestPermission({employeeName, employeeSurname, permissionTypeId}: {employeeName: string, employeeSurname: string, permissionTypeId: number}): Promise<void> {
   return await api.post("/permissions", {
        employee_name: employeeName,
        employee_surname: employeeSurname,
        permission_type_id: permissionTypeId,
    }); 
}

async function modifyPermission({permissionId, employeeName, employeeSurname, permissionTypeId}: {permissionId: number, employeeName: string, employeeSurname: string, permissionTypeId: number}): Promise<void> {
   return await api.put(`/permissions/${permissionId}`, {
        employee_name: employeeName,
        employee_surname: employeeSurname,
        permission_type_id: permissionTypeId,
    }); 
}

export default {
    getPermissions,
    getPermissionTypes,
    requestPermission,
    modifyPermission
};