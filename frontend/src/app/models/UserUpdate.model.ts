export interface UserUpdateModel {
    fullName: string,
    email: string,
    roleId: number,
    playerId?: number | null,
}
