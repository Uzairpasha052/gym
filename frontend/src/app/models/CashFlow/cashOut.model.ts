export interface CashOutModel {
    amount: number;
    description: string;
    paymentDate: string;
}
export interface CashOutListModel extends CashOutModel {
    id: number;
}
