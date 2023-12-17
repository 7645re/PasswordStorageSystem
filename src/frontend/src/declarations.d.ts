declare module "*.module.css";
declare module "*.svg" {
  const content: string;
  export default content;
  import React = require("react");
  export const ReactComponent: React.FC<React.SVGProps<SVGSVGElement>>;
}
