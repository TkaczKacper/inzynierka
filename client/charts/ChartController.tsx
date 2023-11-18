import React, { Component } from "react";
// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";
import { parseDurationExact } from "@/utils/parseDuration";

var CanvasJS = CanvasJSReact.CanvasJS;
var CanvasJSChart = CanvasJSReact.CanvasJSChart;

interface ChartProps {
  data: number[];
}

class ChartController extends Component<ChartProps> {
  render() {
    const dataPoints: { x: number; y: number }[] = [];
    let skip = 0;
    this.props.data.map((value, index) => {
      if (index < 299) {
        return dataPoints.push({ x: index + 1, y: value });
      }
      if (299 <= index && index < 599) {
        if (skip == 0) {
          skip = 4;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (599 <= index && index < 1199) {
        if (skip == 0) {
          skip = 9;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (1199 <= index && index < 2399) {
        if (skip == 0) {
          skip = 14;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (2399 <= index && index < 3599) {
        if (skip == 0) {
          skip = 29;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (3599 <= index && index < 7199) {
        if (skip == 0) {
          skip = 59;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (7199 <= index && index < 10799) {
        if (skip == 0) {
          skip = 149;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (skip == 0) {
        skip = 299;
        return dataPoints.push({ x: index + 1, y: value });
      }
      return skip--;
    });
    const stripLines = axisXstripLines();
    const options = {
      backgroundColor: "rgba(255,255,255,0.1)",
      scales: {},
      title: {
        text: "Power curve",
        fontColor: "#ffffff",
      },
      toolTip: {
        shared: true,
        contentFormatter: function (e: any) {
          let time = parseDurationExact(e.entries[0].dataPoint.x);
          let watt = e.entries[0].dataPoint.y;
          let content = `Time: ${time} <br>`;
          content += `Power: ${watt}W`;
          return content;
        },
      },
      axisX: {
        labelAutoFit: false,
        crosshair: {
          enabled: true,
          label: "",
        },
        stripLines: stripLines,
        logarithmic: true,
        labelFontColor: "#ffffff",
        interval: 1110,
        labelFormatter: function (e: any) {
          return parseDurationExact(e.value);
        },
      },
      axisY: {
        labelFontColor: "#ffffff",
        suffix: "W",
      },
      data: [
        {
          type: "line",
          color: "rgb(46,111,234)",
          dataPoints: dataPoints,
        },
      ],
    };

    return (
      <CanvasJSChart
        options={options}
        /* onRef = {ref => this.chart = ref} */
      />
    );
  }
}
export default ChartController;

type stipLines = {
  value: number;
  color: string;
  thickness: number;
  label: string;
  labelPlacement: string;
  labelFontColor: string;
  labelBackgroundColor: string;
  labelWrap: boolean;
  labelMaxWidth: number;
  opacity: number;
};

const axisXstripLines = () => {
  const index = [2, 5, 10, 30, 60, 180, 300, 600, 1200, 3600, 7200, 18000];

  const stripLines: stipLines[] = [];

  index.map((value, index) => {
    stripLines.push({
      value: value,
      color: "#black",
      thickness: 1,
      label: parseDurationExact(value),
      labelPlacement: "outside",
      labelFontColor: "#ffffff",
      labelBackgroundColor: "transparent",
      labelWrap: false,
      labelMaxWidth: 100,
      opacity: 0.3,
    });
  });

  return stripLines;
};
