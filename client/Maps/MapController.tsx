import { MapContainer, Polyline, TileLayer, useMap } from "react-leaflet";
import React, { useEffect, useState } from "react";
import { polylineDecoder } from "@/Maps/PolylineDecoder";
import L, { Map } from "leaflet";

interface MapProps {
  polyline: string;
}

const MapController: React.FC<MapProps> = ({ polyline }) => {
  const map = useMap();
  const polyline_decoded = polylineDecoder(polyline, 1e5);

  var pol = L.polyline(polyline_decoded, { color: "red" }).addTo(map);

  const bounds = pol.getBounds();
  map.fitBounds(bounds);
  return <></>;
};

export default MapController;
