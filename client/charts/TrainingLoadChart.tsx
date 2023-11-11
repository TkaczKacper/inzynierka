import React, { Component } from "react";
// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";
import { TrainingLoadResponseType } from "@/app/profile/training-load/page";

var CanvasJS = CanvasJSReact.CanvasJS;
var CanvasJSChart = CanvasJSReact.CanvasJSChart;

interface props {
  dataType: number;
  data: TrainingLoadResponseType[];
}
class TrainingLoadChart extends Component<props> {
  render() {
    const options = {
      zoomEnabled: true,
      data: [
        {
          type: "line",
          dataPoints: chartDataLts(this.props.dataType, this.props.data),
        },
        {
          type: "line",
          dataPoints: chartDataSts(this.props.dataType, this.props.data),
        },
        {
          type: "line",
          dataPoints: chartDataSb(this.props.dataType, this.props.data),
        },
      ],
    };

    return (
      <div>
        <CanvasJSChart
          options={options}
          // onRef = {ref => this.chart = ref}
        />
      </div>
    );
  }
}

export default TrainingLoadChart;

const chartDataLts = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y: value.longTermStress,
        });
      });
  }
  return chartData;
};
const chartDataSts = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y: value.shortTermStress,
        });
      });
  }
  return chartData;
};
const chartDataSb = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y: value.stressBalance,
        });
      });
  }
  return chartData;
};

const chartDataTL = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y:
            value.trainingStressScore > 0
              ? value.trainingStressScore
              : value.trainingImpulse,
        });
      });
  }
  return chartData;
};
