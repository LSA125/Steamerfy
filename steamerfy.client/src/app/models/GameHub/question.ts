export class Question {
  questionText: string;
  imageURLAndOption: [string, string][];
  answer: number;
  expireTime: Date;

  constructor(questionText: string = "", imageURLAndOption: [string, string][] = [], answer: number = -1, expireTime: Date = new Date(0)) {
    this.questionText = questionText;
    this.imageURLAndOption = imageURLAndOption;
    this.answer = answer;
    this.expireTime = expireTime;
  }
}
