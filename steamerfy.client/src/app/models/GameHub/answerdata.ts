export class AnswerData {
  SteamId: string;
  AnswerId: number;
  score: number;
  constructor(SteamId: string = "", AnswerId: number = 0, score: number = 0) {
    this.SteamId = SteamId;
    this.AnswerId = AnswerId;
    this.score = score;
  }
}
