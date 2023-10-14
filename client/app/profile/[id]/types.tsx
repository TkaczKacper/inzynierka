export type AthleteInfo = {
  bio: string;
  city: string;
  country: string;
  firstName: string;
  lastName: string;
  profileAvatar: string;
  profileCreatedAt: string;
  profileID: number;
  sex: string;
  state: string;
  weight: number;
};
type RideTotals = {
  achievementCount: number;
  count: number;
  distance: number;
  elapsedTime: number;
  elevationGain: number;
  movingTime: number;
};

export type AthleteStats = {
  allTimeRideTotals: RideTotals;
  biggestClimb: number;
  longestRide: number;
  recentRideTotals: RideTotals;
  ytdRideTotals: RideTotals;
};
