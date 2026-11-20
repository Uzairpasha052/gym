
export interface UserModel {

    id: number,
    fullName: string,
    email: string,
    roleId: number,
    roleName: string,
    playerId?: number | null,
    playerName?: string | null,
    createdAt: string
    isActive: boolean

}
