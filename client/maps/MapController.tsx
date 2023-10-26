import { MapContainer, Polyline, TileLayer, useMap } from "react-leaflet";
import React, { useEffect, useState } from "react";
import { polylineDecoder } from "@/maps/PolylineDecoder";
import L, { LatLngLiteral, Map } from "leaflet";

interface MapProps {
  polyline: string;
  startLatLng?: LatLngLiteral;
  endLatLng?: LatLngLiteral;
}

const MapController: React.FC<MapProps> = ({
  polyline,
  startLatLng,
  endLatLng,
}) => {
  const map = useMap();
  const polyline_decoded = polylineDecoder(polyline, 1e5);

  var pol = L.polyline(polyline_decoded, { color: "red" }).addTo(map);

  if (startLatLng && endLatLng) {
    const startIcon = L.icon({
      iconUrl: "/start-map-icon.png",
      iconSize: [15, 15],
    });

    const endIcon = L.icon({
      iconUrl: "/finish-map-icon.png",
      iconSize: [15, 15],
    });

    L.marker(startLatLng, { icon: startIcon }).addTo(map);
    L.marker(endLatLng, { icon: endIcon }).addTo(map);
  }

  const bounds = pol.getBounds();
  map.fitBounds(bounds);
  return <></>;
};

export default MapController;
