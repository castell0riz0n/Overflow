import Link from "next/link";
import {AcademicCapIcon, MagnifyingGlassIcon} from "@heroicons/react/24/solid";
import {Button, InputGroup, TextField} from "@heroui/react";
import ThemeToggle from "@/components/nav/ThemeToggle";

export default function TopNav() {
    return (
        <header className='p-2 w-full fixed top-0 z-50 border-b bg-white dark:bg-black'>
            <div className="flex px-10 mx-auto">
                <div className="flex items-center gap-6">
                    <Link href="/" className="flex items-center gap-3 mx-h16">
                        <AcademicCapIcon className="size-10 text-secondary"/>
                        <h3 className="text-xl font-semibold uppercase">Overflow</h3>
                    </Link>
                    <nav className="flex gap-3 my-2 text-md text-neutral-500">
                        <Link href="/">About</Link>
                        <Link href="/">Products</Link>
                        <Link href="/">Contact</Link>
                    </nav>
                </div>
                <TextField className="w-full ms-3" name="search">
                    <InputGroup>
                        <InputGroup.Prefix>
                            <MagnifyingGlassIcon className="size-6 text-muted" />
                        </InputGroup.Prefix>
                        <InputGroup.Input type="search" placeholder="search" />
                    </InputGroup>
                </TextField>
                
                <div className="flex basis-1/4 shrink-0 justify-end gap-3">
                    <ThemeToggle />
                    <Button variant={"outline"}>Login</Button>
                    <Button >Register</Button>
                </div>
            </div>
        </header>
    );
}