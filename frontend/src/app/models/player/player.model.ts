/** Player shape returned by the backend (System.Text.Json camel-cases properties). */
export interface PlayerModel {
  id: number;
  name: string;
  fName: string;
  age: string;
  gender: string;
  contact: string;
  history: string;
  reason: string;
  fee: string;
  status: string;
  createdAt: string;
}

/** Fields accepted by POST /api/player. */
export interface CreatePlayerRequest {
  name: string;
  fName: string;
  age: string;
  gender: string;
  contact: string;
  history: string;
  reason: string;
  fee: string;
}

/** Editable fields accepted by PUT /api/player/{id}. */
export interface UpdatePlayerRequest extends CreatePlayerRequest {
  status: string;
}
