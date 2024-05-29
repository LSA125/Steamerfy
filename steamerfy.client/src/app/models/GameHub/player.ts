export class Player {
  Username: string;
  ProfileUrl: string;
  AvatarUrl: string;
  SteamId: string;
  isHost: boolean;
  isUser: boolean;
  Score: number;

  constructor(username: string = "", profileUrl: string = "", avatarUrl: string = "", steamId: string = "", isHost: boolean = false, isUser: boolean = false, score: number = 0) {
    this.Username = username;
    this.ProfileUrl = profileUrl;
    this.AvatarUrl = avatarUrl;
    this.SteamId = steamId;
    this.isHost = isHost;
    this.isUser = isUser;
    this.Score = score;
  }
}
