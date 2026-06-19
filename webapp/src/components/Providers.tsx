'use client';
import {ReactNode} from "react";
import {ThemeProvider} from "next-themes";

export default function Providers({children}: {children: ReactNode}) {
    return (
        <div className="flex flex-col h-full">
            <ThemeProvider
                attribute="class"
                defaultTheme="light"
            >
                {children}
            </ThemeProvider>
        </div>
    );
}