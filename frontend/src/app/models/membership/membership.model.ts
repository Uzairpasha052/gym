export interface CreateMembershipRequest {
  playerId: number;
  durationMonths: number;
}

export interface MembershipRecord {
  id: number;
  durationMonths: number;
  startDate: string;
  endDate: string;
  finalAmount: number;
  discountAmount: number;
  status: string;
}

export interface ExpiringMember {
  playerId: number;
  playerName: string;
  contact: string;
  expiryDate: string | null;
  coverageType: string;
}

export interface MembershipPurchaseResponse {
  message: string;
  data: {
    id: number;
    playerId: number;
    playerName: string;
    durationMonths: number;
    startDate: string;
    endDate: string;
    monthlyFee: number;
    originalAmount: number;
    discountAmount: number;
    finalAmount: number;
    status: string;
    cashInId: number | null;
    createdAt: string;
  };
}
