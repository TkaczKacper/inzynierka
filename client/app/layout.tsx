import "./globals.css";
import type { Metadata } from "next";
import dynamic from "next/dynamic";
import { Inter } from "next/font/google";

const inter = Inter({ subsets: ["latin"] });

const Navbar = dynamic(() => import("./navbar"), { ssr: false });

export const metadata: Metadata = {
   title: "Cycling Analytics",
   description: "inzynierka",
};

export default function RootLayout({
   children,
}: {
   children: React.ReactNode;
}) {
   return (
      <html lang="en">
         <body className={inter.className}>
            <div style={{ height: "50px" }}>
               <Navbar />
            </div>
            {children}
         </body>
      </html>
   );
}
