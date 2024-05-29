import { AnswerData } from "./answerdata";
import { Player } from "./player";
import { Question } from "./question";

export class GameState
{
 constructor(public steamId: string = "",
    public LobbyId: number,
    public CurrentQuestion: Question,
    public Players: Player[],
    public AnswerData: AnswerData[],
    public HostSteamId: string
  ) { }
}
