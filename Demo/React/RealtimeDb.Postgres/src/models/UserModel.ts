export class UserModel {
  UserId: number;
  UserName: string;

  constructor(userId: number, userName: string) {
    this.UserId = userId;
    this.UserName = userName;
  }
}
