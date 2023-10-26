import { LatLngLiteral } from "leaflet";

export function polylineDecoder(encoded: string, mul: number) {
  const inv = 1.0 / mul;
  const decoded: LatLngLiteral[] = [];
  let prev = [0, 0];

  let i = 0;

  while (i < encoded.length) {
    var latlng = [0, 0];

    for (let j = 0; j < 2; j++) {
      let shift = 0;
      let byte = 0x20;

      while (byte >= 0x20) {
        byte = encoded.charCodeAt(i++) - 63;
        latlng[j] |= (byte & 0x1f) << shift;
        shift += 5;
      }

      latlng[j] =
        prev[j] + (latlng[j] & 1 ? ~(latlng[j] >> 1) : latlng[j] >> 1);
      prev[j] = latlng[j];
    }
    decoded.push({ lat: latlng[0] * inv, lng: latlng[1] * inv });
  }
  console.log(decoded);
  return decoded;
}
