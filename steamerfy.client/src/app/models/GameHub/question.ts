export class Question {
  QuestionText: string;
  QuestionURL: string;
  ImageURLAndOption: [string, string][];
  Answer: number;
  ExpireTime: Date;

  constructor(questionText: string = "", questionURL:string = "", imageURLAndOption: [string, string][] = [], answer: number = -1, expireTime: Date = new Date(0)) {
    this.QuestionText = questionText;
    this.QuestionURL = questionURL;
    this.ImageURLAndOption = imageURLAndOption;
    this.Answer = answer;
    this.ExpireTime = expireTime;
  }
}
