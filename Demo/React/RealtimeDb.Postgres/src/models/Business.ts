export class Business {
  Id: number;
  Name: string;
  Rating: number;

  constructor(id: number, name: string = "", rating: number = 0) {
    this.Id = id;
    this.Name = name;
    this.Rating = rating;
  }
}
