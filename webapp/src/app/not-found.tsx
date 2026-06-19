'use client'

import {Button} from "@heroui/react";
import {useRouter} from "next/navigation";
import Link from "next/link";


export default function NotFound() {
    const router = useRouter();

    return (
        <div className="h-full flex items-center justify-center">
            <div className="text-center space-y-6">
                <h1 className="text-5xl font-bold">404 - Page Not Found</h1>
                <p className="text-lg text-foreground-500">Sorry, the page you are looking for doesn't exist</p>
                <Link href="/">
                    <Button className="btn btn-primary"
                    >Go back to home</Button>
                </Link>
            </div>
        </div>
    );
}