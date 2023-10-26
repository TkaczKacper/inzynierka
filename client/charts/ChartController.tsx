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
    this.props.data.map((value, index) => {
      return dataPoints.push({ x: index + 1, y: value });
    });
    console.log(this.props.data);
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
        stripLines: [
          {
            value: 2,
            color: "black",
            thickness: 1,
          },
          {
            value: 5,
            color: "black",
            thickness: 1,
          },
          {
            value: 10,
            color: "black",
            thickness: 1,
          },
          {
            value: 30,
            color: "black",
            thickness: 1,
          },
          {
            value: 60,
            color: "black",
            thickness: 1,
          },
          {
            value: 180,
            color: "black",
            thickness: 1,
          },
          {
            value: 300,
            color: "black",
            thickness: 1,
          },
          {
            value: 600,
            color: "black",
            thickness: 1,
          },
          {
            value: 1200,
            color: "black",
            thickness: 1,
          },
        ],
        logarithmic: true,
        // interval: 0.2,
        interval: Math.log10(1.6),
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
