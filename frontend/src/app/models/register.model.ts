export interface registerModel {
    fullName: string,
    email: string,
    password: string,
    roleId: number | null,
    playerId?: number | null
}
