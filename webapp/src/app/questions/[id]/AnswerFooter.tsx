import {Answer} from "@/lib/types";
import {Avatar} from "@heroui/react";

export default function AnswerFooter({answer}: {answer: Answer}) {
    return (
        <div className='flex justify-end mt-4'>
            <div className='flex flex-col basis-2/5 bg-accent/10 px-3 py-2 gap-2 rounded-lg'>
                <span className='text-sm font-extralight'>answered {answer.createdAt}</span>
                <div className='flex flex-row items-center gap-3'>
                    <Avatar className='h-6 w-6' color='warning' variant="soft" >
                        <Avatar.Fallback>{answer.userDisplayName.charAt(0)}</Avatar.Fallback>
                    </Avatar>
                    <div className='flex flex-col items-center'>
                        <span>{answer.userDisplayName}</span>
                        <span className='text-sm self-start font-semibold'>42</span>
                    </div>
                </div>
            </div>
        </div>
    );
}