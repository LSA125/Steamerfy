export class Player {
  username: string;
  profileUrl: string;
  avatarUrl: string;
  steamId: string;
  isHost: boolean;
  isUser: boolean;
  score: number;

  constructor(username: string = "", profileUrl: string = "", avatarUrl: string = "", steamId: string = "", isHost: boolean = false, isUser: boolean = false, score: number = 0) {
    this.username = username;
    this.profileUrl = profileUrl;
    this.avatarUrl = avatarUrl;
    this.steamId = steamId;
    this.isHost = isHost;
    this.isUser = isUser;
    this.score = score;
  }
}
