
import {ArrowDownCircleIcon, ArrowUpCircleIcon, CheckIcon} from "@heroicons/react/24/outline";
import {Button} from "@heroui/react";

type Props = {
    accepted?: boolean;
}

export default function VotingButtons({accepted}: Props) {
    return (
        <div className='shrink-0 flex flex-col gap-3 items-center justify-start mt-4'>
            <Button isIconOnly
                    variant='ghost'>
                <ArrowUpCircleIcon className='size-8'/>
            </Button>
            <span className='text-xl font-semibold'>0</span>
            <Button isIconOnly
                    variant='ghost'>
                <ArrowDownCircleIcon className='size-8'/>
            </Button>
            {accepted && (
                <Button isIconOnly
                        variant='ghost'>
                    <CheckIcon
                        className='size-8 text-success'
                        strokeWidth={4}
                    />
                </Button>
            )}
        </div>
    );
}