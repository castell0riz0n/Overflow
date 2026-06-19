'use client'


import {ListBox, Select} from "@heroui/react";

type Props = {
    answerCount: number;
}

export default function AnswersHeader({answerCount}: Props) {
    return (
        <div className='flex items-center justify-between pt-3 w-full px-6'>
            <div className='text-2xl'>{answerCount} {answerCount === 1 ? ' Answer' : ' Answers'}</div>
            <div className='flex items-center gap-3 justify-end w-[50%] ml-auto'>
                
                {/*<Select*/}
                {/*    aria-label='select sorting'*/}
                {/*    defaultSelectedKeys={['highScore']}*/}
                {/*>*/}
                {/*    <Select.Item key='highScore'>Highest score (default)</Select.Item>*/}
                {/*    <Select.Item key='created'>Date created</Select.Item>*/}
                {/*</Select>*/}

                <Select placeholder="select sorting" defaultValue={'highScore'}>
                    <Select.Trigger>
                        <Select.Value />
                        <Select.Indicator />
                    </Select.Trigger>
                    <Select.Popover>
                        <ListBox>
                            <ListBox.Item id="highScore" textValue="highScore">
                                Highest score (default)
                                <ListBox.ItemIndicator />
                            </ListBox.Item>
                            <ListBox.Item id="delaware" textValue="highScore">
                                Date created
                                <ListBox.ItemIndicator />
                            </ListBox.Item>
                        </ListBox>
                    </Select.Popover>
                </Select>
            </div>
        </div>
    );
}