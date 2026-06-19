'use client';

import Link from "next/link";
import { Button, Tabs, Tab } from "@heroui/react";
import {useRouter} from "next/navigation";

type Props = {
    tag?: string;
    total: number;
};

export default function QuestionsHeader({ tag, total }: Props) {

    const router = useRouter();
    
    const tabs = [
        { key: "newest", label: "Newest" },
        { key: "active", label: "Active" },
        { key: "unanswered", label: "Unanswered" },
    ];

    return (
        <div className="flex flex-col w-full border-b gap-4 pb-4">
            <div className="flex justify-between px-6">
                <div className="text-3xl font-semibold">
                    {tag ? `[${tag}]` : "Newest questions"}
                </div>

                <Button
                    onPress={() => router.push("/questions/ask")}
                    variant="secondary"
                    size={"md"}
                >
                    Ask Question
                </Button>
            </div>

            <div className="flex items-center justify-between px-6">
                <div>
                    {total} {total === 1 ? "Question" : "Questions"}
                </div>

                <Tabs className="w-full max-w-md">
                    <Tabs.ListContainer>
                        <Tabs.List aria-label="Options">
                            {tabs.map((item) => (
                                <Tabs.Tab key={item.key} id={item.key}>
                                    {item.label}
                                    <Tabs.Indicator />
                                </Tabs.Tab> 
                            ))}
                        </Tabs.List>
                    </Tabs.ListContainer>
                </Tabs>

                {/*<Tabs aria-label="Questions filter tabs">*/}
                {/*    {tabs.map((item) => (*/}
                {/*        <Tab key={item.key} title={item.label} />*/}
                {/*    ))}*/}
                {/*</Tabs>*/}
            </div>
        </div>
    );
}
