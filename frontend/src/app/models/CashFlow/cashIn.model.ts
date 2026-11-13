
export interface CashInModel {
    playerId: number;
    amount: number;
    paymentDate: string;
}
export interface CashInListModel extends CashInModel {
    id: number;
    playerName: string;
}
