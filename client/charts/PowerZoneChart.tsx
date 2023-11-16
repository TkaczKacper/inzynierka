import { Component } from "react";

// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";
import { powerTimeInZoneType } from "@/app/profile/[id]/types";
import { parseDurationExact } from "@/utils/parseDuration";

var CanvasJsChart = CanvasJSReact.CanvasJSChart;

type zonesType = {
  dateAdded: string;
  ftp: number;
  zone1: number;
  zone2: number;
  zone3: number;
  zone4: number;
  zone5: number;
  zone6: number;
  zone7: number;
};

interface PowerProps {
  data: powerTimeInZoneType;
  zones: zonesType;
}

class PowerZoneChart extends Component<PowerProps> {
  render() {
    const zones = [
      this.props.zones.zone1,
      this.props.zones.zone2,
      this.props.zones.zone3,
      this.props.zones.zone4,
      this.props.zones.zone5,
      this.props.zones.zone6,
      this.props.zones.zone7,
    ];
    const timeInZone = [
      this.props.data.timeInZ1,
      this.props.data.timeInZ2,
      this.props.data.timeInZ3,
      this.props.data.timeInZ4,
      this.props.data.timeInZ5,
      this.props.data.timeInZ6,
      this.props.data.timeInZ7,
    ];

    const options = {
      backgroundColor: "rgba(0,0,0,0)",
      toolTip: {
        shared: true,
        reversed: true,
        contentFormatter: function (e: any) {
          let label = e.entries[0].dataPoint.label;
          let index = e.entries[0].dataPoint.x;
          if (index === 6) {
            return `${label}: ${zones[index]}W+`;
          }
          return `${label}: ${zones[index]} - ${zones[index + 1] - 1}W`;
        },
      },
      axisX: {
        valueFormatString: "",
        tickLength: 0,
        lineThickness: 0,
        crosshair: {
          enabled: true,
          label: "",
        },
        gridThickness: 0,
        labelPlacement: "inside",
        labelFontColor: "#575757",
        labelFormatter: function (e: any) {
          return parseDurationExact(timeInZone[e.value]);
        },
      },
      dataPointWidth: 38,
      axisY: {
        valueFormatString: "",
        lineThickness: 0,
        gridThickness: 0,
        tickLength: 0,
        labelFormatter: function (e: any) {
          return "";
        },
      },
      data: [
        {
          type: "bar",
          dataPoints: [
            {
              label: "Recovery",
              y: this.props.data.timeInZ1,
              color: "#bdeea8",
            },
            { label: "Base", y: this.props.data.timeInZ2, color: "#f4e9af" },
            { label: "Tempo", y: this.props.data.timeInZ3, color: "#ffd8af" },
            {
              label: "Threshold",
              y: this.props.data.timeInZ4,
              color: "#ffc7bf",
            },
            { label: "VO2 Max", y: this.props.data.timeInZ5, color: "#ffbde1" },
            {
              label: "Anaerobic",
              y: this.props.data.timeInZ6,
              color: "#e4bced",
            },
            {
              label: "Neuromuscular",
              y: this.props.data.timeInZ7,
              color: "#b2bff6",
            },
          ],
        },
      ],
    };

    return (
      <div>
        <CanvasJsChart
          options={options}
          containerProps={{ width: "100%", height: "280px" }}
        />
      </div>
    );
  }
}

export default PowerZoneChart;
