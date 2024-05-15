export interface Question {
  questionText: string;
  imageURLAndOption: [string, string][];
  answer: number;
  expireTime: Date;
}
