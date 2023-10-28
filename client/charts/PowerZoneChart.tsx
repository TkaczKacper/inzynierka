import { Component } from "react";

// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";
import { powerTimeInZoneType } from "@/app/profile/[id]/types";

var CanvasJsChart = CanvasJSReact.CanvasJSChart;

type zonesType = {
  dateAdded: string;
  hrRest: number;
  hrMax: number;
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
    console.log(this.props.data);
    console.log(this.props.zones);
    const options = {
      data: [
        {
          type: "bar",
          dataPoints: [
            { label: "Zone7", y: this.props.data.timeInZ7, color: "#b2bff6" },
            { label: "Zone6", y: this.props.data.timeInZ6, color: "#e4bced" },
            { label: "Zone5", y: this.props.data.timeInZ5, color: "#ffbde1" },
            { label: "Zone4", y: this.props.data.timeInZ4, color: "#ffc7bf" },
            { label: "Zone3", y: this.props.data.timeInZ3, color: "#ffd8af" },
            { label: "Zone2", y: this.props.data.timeInZ2, color: "#f4e9af" },
            { label: "Zone1", y: this.props.data.timeInZ1, color: "#bdeea8" },
          ],
        },
      ],
    };

    return (
      <div>
        <CanvasJsChart options={options} />
      </div>
    );
  }
}

export default PowerZoneChart;
