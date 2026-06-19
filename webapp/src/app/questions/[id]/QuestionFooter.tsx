import {Question} from "@/lib/types";
import {Avatar, Chip} from "@heroui/react";
import Link from "next/link";

type Props = {
    question: Question;
}
export default function QuestionFooter({question}: Props) {
    return (
        <div className='flex justify-between mt-2'>
            <div className='flex flex-col self-end'>
                <div className="flex gap-2">
                    {question.tagSlugs.map((tag: string) => (
                        <Link key={tag} href={`/questions?tag=${tag}`}>
                            <Chip
                                variant="secondary"
                            >
                                {tag}
                            </Chip>
                        </Link>

                    ))}
                </div>
            </div>

            <div className='flex flex-col basis-2/5 bg-accent/10 px-3 py-2 gap-2 rounded-lg'>
                <span className='text-sm font-extralight'>asked {question.createdAt}</span>
                <div className='flex items-center gap-3'>
                    <Avatar className='h-6 w-6' variant="soft" color='warning'>
                        <Avatar.Fallback>
                            {question.askerDisplayName.charAt(0)}
                        </Avatar.Fallback>
                    </Avatar>
                    <div className='flex flex-col items-center'>
                        <span>{question.askerDisplayName}</span>
                        <span className='text-sm self-start font-semibold'>42</span>
                    </div>
                </div>
            </div>
        </div>
    );
}