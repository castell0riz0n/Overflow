"use client";

import {HomeIcon, QuestionMarkCircleIcon, TagIcon, UserIcon} from "@heroicons/react/24/solid";
import {ListBox} from "@heroui/react";
import {usePathname, useRouter} from "next/navigation";

export default function SideMenu() {
    const router = useRouter();
    
    const pathname = usePathname();
    
    const navLinks = [
        {key: "home", icon: HomeIcon, text: "Home", href: '/'},
        {key: "questions", icon: QuestionMarkCircleIcon, text: "Questions", href: '/questions'},
        {key: "tags", icon: TagIcon, text: "Tags", href: '/tags'},
        {key: "session", icon: UserIcon, text: "User Session", href: '/session'},
    ];

    return (
        <ListBox 
            items={navLinks} 
            aria-label="nav links" 
            className="sticky top-20 ms-6" 
            selectionMode="single"
            onAction={(key) => {
                const item = navLinks.find(x => x.key === key);
                if (item) router.push(item.href);
            }}
        >
            {({ key, href, icon: Icon, text }) => {
                const isActive = pathname === href;
                return (
                <ListBox.Item key={key} textValue={text}
                              href={href}
                              aria-label={text}
                              aria-labelledby={key}
                              aria-describedby={text}
                >
                    <div className="flex items-center gap-3">
                        <Icon className={`w-5 h-5 ${isActive ? "text-accent" : "text-primary"}`}/>
                        <span className={isActive ? 'text-accent' : 'text-primary'} >{text}</span>
                    </div>
                </ListBox.Item>
                )
            }}
        </ListBox>
    );
}
