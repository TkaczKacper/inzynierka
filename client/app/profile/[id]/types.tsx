//TODO zmienic miejsce tego pliku + rozdzielic na podpliki (?)
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
  seconds: number;
  minutes: number;
  hours: number;
};

export type AthleteStats = {
  allTimeRideTotals: RideTotals;
  biggestClimb: number;
  longestRide: number;
  recentRideTotals: RideTotals;
  ytdRideTotals: RideTotals;
};

export type MonthlySummary = {
  month: number;
  totalCalories: number;
  totalDistance: number;
  totalElapsedTime: number;
  totalElevationGain: number;
  totalMovingTime: number;
  trainingLoad: number;
  year: number;
  fillHeight: number;
};

export type ActivityLap = {
  avgCadence: number;
  avgHeartRate: number;
  avgSpeed: number;
  avgWatts: number;
  distance: number;
  elapsedTime: number;
  endIdx: number;
  lapIndex: number;
  maxHeartRate: number;
  maxSpeed: number;
  movingTime: number;
  startDate: Date;
  startIdx: number;
  totalElevationGain: number;
};

export type hrTimeInZoneType = {
  timeInZ1: number;
  timeInZ2: number;
  timeInZ3: number;
  timeInZ4: number;
  timeInZ5: number;
};

export type powerTimeInZoneType = {
  timeInZ1: number;
  timeInZ2: number;
  timeInZ3: number;
  timeInZ4: number;
  timeInZ5: number;
  timeInZ6: number;
  timeInZ7: number;
};

export type Activity = {
  id: number;
  achievements: number;
  altitude: number[];
  avgCadence: number;
  avgHeartRate: number;
  avgSpeed: number;
  avgTemp: number;
  avgWatts: number;
  cadence: number[];
  calories: number;
  detailedPolyline: string;
  deviceName: string;
  distance: number[];
  elapsedTime: number;
  elevationHigh: number;
  elevationLow: number;
  endLatLng: number[];
  gear: string;
  gradeSmooth: number[];
  hasPowerMeter: boolean;
  heartRate: number[];
  intensity: number;
  laps: ActivityLap[];
  lat: number[];
  lng: number[];
  maxCadence: number;
  maxHeartRate: number;
  maxSpeed: number;
  maxTemperature: number;
  maxWatts: number;
  moving: boolean[];
  movingTime: number;
  effectivePower: number;
  startDate: Date;
  startLatLng: number[];
  stravaActivityID: number;
  summaryPolyline: string;
  timeStream: number[];
  title: string;
  totalDistance: number;
  totalElevationGain: number;
  trainer: boolean;
  trainingLoad: number;
  trimp: number;
  type: string;
  sportType: string;
  variabilityIndex: number;
  velocity: number[];
  watts: number[];
  weightedAvgWatts: number[];
  powerCurve: number[];
  hrTimeInZone: hrTimeInZoneType;
  powerTimeInZone: powerTimeInZoneType;
};
