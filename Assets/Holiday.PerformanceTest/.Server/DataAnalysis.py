from sys import argv
from glob import glob

def main():
    if argv.count == 1:
        return

    output_file_name = argv[1][:-1] + '.csv'
    tmp_data = []
    for input_file_name in glob(argv[1]):
        tmp_data.append(analysis(input_file_name))

    tmp_data.sort(key=lambda t: int(t[0]))
    data = [','.join(t) + '\n' for t in tmp_data]
    data.insert(0, 'num,max,min\n')

    f = open(output_file_name, 'w', encoding='UTF-8')
    f.writelines(data)
    f.close()


def analysis(file_name):
    if file_name[-6] == 'n':
        file_num = file_name[-5]
    else:
        file_num = file_name[-6:-4]

    f = open(file_name, 'r', encoding='UTF-8')
    data = f.readlines()
    f.close()

    target_data = data[300:-300]
    memories = [s.split(' ')[4] for s in target_data]
    max_memory = max(memories)
    min_memory = min(memories)

    return file_num, max_memory, min_memory


if __name__ == '__main__':
    main()
