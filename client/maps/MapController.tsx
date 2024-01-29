"use client";

import { useMap } from "react-leaflet";
import React from "react";
import { polylineDecoder } from "@/maps/PolylineDecoder";
import L, { control, LatLngLiteral } from "leaflet";
import zoom = control.zoom;

interface MapProps {
  polyline: string;
  startLatLng: number[];
  endLatLng: number[];
}

const MapController: React.FC<MapProps> = ({
  polyline,
  startLatLng,
  endLatLng,
}) => {
  const map = useMap();

  map.eachLayer((layer) => {
    if (layer instanceof L.Marker || layer instanceof L.Path) {
      map.removeLayer(layer);
    }
  });

  const polyline_decoded = polylineDecoder(polyline, 1e5);
  const startCoordinates: LatLngLiteral = {
    lat: startLatLng[0],
    lng: startLatLng[1],
  };
  const endCoordinates: LatLngLiteral = {
    lat: endLatLng[0],
    lng: endLatLng[1],
  };

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

    L.marker(startCoordinates, { icon: startIcon }).addTo(map);
    L.marker(endCoordinates, { icon: endIcon }).addTo(map);
  }

  const bounds = pol.getBounds();
  map.fitBounds(bounds);

  return null;
};

export default MapController;
