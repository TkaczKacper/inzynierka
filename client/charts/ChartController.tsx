import React, { Component } from "react";
// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";
import { parseDurationNumeric } from "@/utils/parseDuration";

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
      if (index < 300) {
        return dataPoints.push({ x: index + 1, y: value });
      }
      if (300 <= index && index < 600) {
        if (skip == 0) {
          skip = 4;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (600 <= index && index < 1200) {
        if (skip == 0) {
          skip = 9;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (1200 <= index && index < 2400) {
        if (skip == 0) {
          skip = 14;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (2400 <= index && index < 3600) {
        if (skip == 0) {
          skip = 29;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (3600 <= index && index < 7200) {
        if (skip == 0) {
          skip = 59;
          return dataPoints.push({ x: index + 1, y: value });
        }
        return skip--;
      }
      if (3600 * 2 <= index && index < 3600 * 3) {
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
    console.log(this.props.data);
    console.log(dataPoints);
    const stripLines = axisXstripLines();
    const options = {
      scales: {},
      title: {
        text: "Power curve",
      },
      axisX: {
        labelAutoFit: false,
        crosshair: {
          enabled: true, //disable here
        },
        stripLines: stripLines,
        logarithmic: true,
        interval: 1110,
        labelFormatter: function (e: any) {
          return parseDurationNumeric(e.value);
        },
      },
      data: [
        {
          type: "line",
          toolTipContent: "Time: {x}s, Watts: {y}",
          dataPoints: dataPoints,
        },
      ],
    };

    return (
      <div>
        <CanvasJSChart
          options={options}
          /* onRef = {ref => this.chart = ref} */
        />
      </div>
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
};

const axisXstripLines = () => {
  const index = [2, 5, 10, 30, 60, 180, 300, 600, 1200, 3600, 7200, 18000];

  const stripLines: stipLines[] = [];

  index.map((value, index) => {
    stripLines.push({
      value: value,
      color: "black",
      thickness: 1,
      label: parseDurationNumeric(value),
      labelPlacement: "outside",
      labelFontColor: "black",
      labelBackgroundColor: "transparent",
      labelWrap: false,
      labelMaxWidth: 100,
    });
  });

  return stripLines;
};
