import { Numeric } from "type-fest/source/numeric";
import { LatLngLiteral } from "leaflet";

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
  endLatLng: LatLngLiteral;
  gear: string;
  gradeSmooth: number[];
  hasPowerMeter: boolean;
  heartRate: number[];
  intensityFactor: number;
  laps: ActivityLap[];
  lat: number[];
  lng: number[];
  maxHeartRate: number;
  maxSpeed: number;
  maxWatts: number;
  moving: boolean[];
  movingTime: number;
  normalizedPower: number;
  startDate: Date;
  startLatLng: LatLngLiteral;
  stravaActivityID: number;
  summaryPolyline: string;
  timeStream: number[];
  title: string;
  totalDistance: number;
  totalElevationGain: number;
  trainer: boolean;
  tss: number;
  variabilityIndex: number;
  velocity: number[];
  watts: number[];
  weightedAvgWatts: number[];
  powerCurve: number[];
  hrTimeInZone: hrTimeInZoneType;
  powerTimeInZone: powerTimeInZoneType;
};
