import type Permission from "./permission";

export default interface IPermissionsResponse {
    data: Permission[];
    has_next_page: boolean;
}