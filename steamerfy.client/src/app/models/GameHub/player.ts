export class Player {

  constructor(public Username: string,
    public ProfileUrl: string,
    public AvatarUrl: string,
    public SteamId: string,
    public isHost: boolean = false,
    public isUser: boolean = false,
    public SelectedAnswer: number = -1,
    public Score: number = 0) {

  }
}
