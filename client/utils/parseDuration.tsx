export const parseDurationExact = (time: number) => {
  const hours = Math.floor(time / 3600);
  const minutes = Math.floor((time - hours * 3600) / 60);
  const seconds = Math.floor(time - hours * 3600 - minutes * 60);

  return `${hours > 0 ? hours + "h" : ""} ${minutes > 0 ? minutes + "m" : ""} ${
    seconds > 0 ? seconds + "s" : ""
  }`;
};

export const parseDurationNumeric = (time: number) => {
  const hours = Math.floor(time / 3600);
  const minutes = Math.floor((time - hours * 3600) / 60);
  const seconds = Math.floor(time - hours * 3600 - minutes * 60);

  let minute_string: string = minutes.toString();
  if (minutes < 10) {
    minute_string = "0" + minutes;
  }
  let seconds_string: string = seconds.toString();
  if (seconds < 10) {
    seconds_string = "0" + seconds;
  }
  return `${hours > 0 ? hours + ":" : ""}${
    minute_string + ":"
  }${seconds_string}`;
};
