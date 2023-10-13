import "./globals.css";
import type { Metadata } from "next";
import dynamic from "next/dynamic";
import { Inter } from "next/font/google";
import { UserContextProvider } from "@/contexts/UserContextProvider";

const inter = Inter({ subsets: ["latin"] });

const Navbar = dynamic(() => import("@/app/(navbar)/navbar"), { ssr: false });

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
        <UserContextProvider>
          <div style={{ height: "50px" }}>
            <Navbar />
          </div>
          {children}
        </UserContextProvider>
      </body>
    </html>
  );
}
